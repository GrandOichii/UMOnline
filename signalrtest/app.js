import { WebSocket } from 'ws';
import { HubConnectionBuilder } from '@microsoft/signalr'

const ADDR = 'localhost';
const PORT = 5156;
const SIGNALR_ENDPOINT = 'Connect';
const WS_ENDPOINT = 'api/v1/Matches/Connect'

async function delay(ms) {
    await new Promise(res => setTimeout(res, ms));
}

async function main() {
    const connection = new HubConnectionBuilder()
        .withUrl(`http://${ADDR}:${PORT}/${SIGNALR_ENDPOINT}`)
        .build();

    connection.on('RegistrationError', async (errMsg) => {
        console.log(`Failed to register: ${errMsg}`);
        await connection.stop();
    });

    await connection.start();
    await connection.invoke('RegisterName', 'Amogus');
    let connectEndpoint = await connection.invoke('CreateMatch', {
        title: 'match1'
    });

    if (connectEndpoint.startsWith('err:')) {
        console.log(connectEndpoint);
        await connection.stop();
        ws.close();
        return;
    }

    const ws = new WebSocket(`ws://${ADDR}:${PORT}/${WS_ENDPOINT}?connectStr=${connectEndpoint}`)
    // const ws = new WebSocket(`ws://${ADDR}:${PORT}/${WS_ENDPOINT}?connectStr=hehe`)

    ws.on('message', msg => {
        console.log(msg.toString());
    })

    ws.on('connection', () => {
        console.log('connection');

    });

    ws.on('close', (code, reason) => {
        console.log((code, reason.toString()));
    })

    ws.on('error', (err) => {
        console.log(err);
    });

    while (true) {
        await delay(1000);
    }
}

await main();

