import { WebSocket } from 'ws';
import { HubConnectionBuilder } from '@microsoft/signalr'

const ADDR = 'localhost';
const PORT = 5156;
const SIGNALR_ENDPOINT = 'Matches';
const MATCH_WS_ENDPOINT = 'api/v1/Matches/Connect'
const MODE = process.argv[2];
const DECK_NAME = process.argv[3];
const MATCH_ID = process.argv[4];

async function delay(ms) {
    await new Promise(res => setTimeout(res, ms));
}

async function main() {
    const connection = new HubConnectionBuilder()
        .withUrl(`http://${ADDR}:${PORT}/${SIGNALR_ENDPOINT}`)
        .build();

    connection.on('ChatUpdate', msg => {
        console.log(msg); 
    });

    connection.on('UpdateTables', matches => {
        console.log(`[UpdateTables] ${matches.length}`);
        console.log(JSON.stringify(matches, null, 4));
    });

    let running = true;
    connection.onclose(async err => {
        await connection.stop();
        running = false;
    });

    await connection.start();
    await connection.invoke('RegisterName', (MODE == 'create' ? 'C1' : 'C2'));

    let matchId;
    if (MODE == 'create') {
        matchId = await connection.invoke('CreateMatch', {
            title: 'match1',
            matchConfigName: 'Seed 0 tester',
            allowedLoadouts: [
                'Medusa',
                'King Arthur'
            ]
        });
        if (matchId.startsWith('err:')) {
            console.log(matchId);
            await connection.stop();
            return;
        }

        console.log(`Created match with id = ${matchId}`);
    } else if (MODE == 'connect') {
        matchId = MATCH_ID;
    } else {
        throw new Error('Invalid mode: ' + MODE);
    }

    let connectEndpoint = await connection.invoke('Connect', matchId);
    if (connectEndpoint.startsWith('err:')) {
        console.log('Failed to connect!');
        console.log(connectEndpoint);
        await connection.stop();
        return;
    }

    console.log(`Connected to match with id = ${matchId}`);

    console.log('Received WS connection endpoint: ' + connectEndpoint);
    const matchWS = new WebSocket(`ws://${ADDR}:${PORT}/${MATCH_WS_ENDPOINT}?connectStr=${connectEndpoint}`);

    matchWS.on('message', msg => {
        console.log('[WS MESSAGE]');
        console.log(msg.toString());
    });

    matchWS.on('close', async (code, reason) => {
        console.log('[WS CLOSE]');
        console.log((code, reason.toString()));

        await connection.stop();
        matchWS.close();
    });

    matchWS.on('error', async (err) => {
        console.log('[WS ERROR]');
        console.log(err);

        await connection.stop();
        matchWS.close();
    });

    let err = await connection.invoke('SelectLoadout', matchId, DECK_NAME);
    if (err != '') {
        console.log('Failed to select loadout');
        console.log(err);
        await connection.stop();
        return;
    }
    err = await connection.invoke('SelectTeam', matchId, (MODE == 'create' ? 0 : 1));
    if (err != '') {
        console.log('Failed to select team');
        console.log(err);
        await connection.stop();
        return;
    }
    
    while (true) {
        let canStart = await connection.invoke('CanStart', matchId);
        if (canStart) break;

        console.log('Cant start match yet, waiting for 1000ms');
        await delay(1000);
    }

    // console.log('Starting match');

    await connection.send('Start', matchId);

    while (running) {
        await delay(1000);
    }
}

await main();

