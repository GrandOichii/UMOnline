using Godot;
using System;
using System.Net.Http.Json;

public partial class DistantMatchesTab : Control
{
    [Export]
    public string DefaultAddress { get; set; }
    
    #region Nodes

    [Export]
    public LocalRepository RepoNode { get; set; }
    [Export]
    public ServerConnection ServerConnectionNode { get; set; }
    [ExportGroup("Nodes")]
    [Export]
    public Control ConnectionFormNode { get; set; }
    [Export]
    public Control ConnectionDisplayNode { get; set; }
    [Export]
    public LineEdit ServerAddressEditNode { get; set; }
    [Export]
    public LineEdit NameEditNode { get; set; }
    [Export]
    public AcceptDialog ConnectionErrorDialogNode { get; set; }
    [Export]
    public HttpRequest ConnectionRequestNode { get; set; }
    [Export]
    public Button ConnectButtonNode { get; set; }
    [Export]
    public AcceptDialog OutdatedContentDialogNode { get; set; }
    [Export]
    public Window ContentSyncWaitDialogNode { get; set; }
    [Export]
    public AcceptDialog ContentUpdateFailDialogNode { get; set; }
    [Export]
    public AcceptDialog FinishedContentUpdateDialog { get; set; }

    #endregion

    private void CheckCanPressConnect()
    {
        ConnectButtonNode.Disabled = true;
        if (ServerAddressEditNode.Text.Length == 0) return;
        if (NameEditNode.Text.Length == 0) return;

        ConnectButtonNode.Disabled = false;
    }

    public override void _Ready()
    {
        ServerConnectionNode.ContentUpdateFinished += OnServerConnectionNodeContentUpdateFinished;
        ServerConnectionNode.ContentUpdateFailed += OnServerConnectionNodeContentUpdateFailed;

        ConnectionDisplayNode.Hide();
        ConnectionFormNode.Show();

        var state = RepoNode.GetAppState();

        ServerAddressEditNode.Text = state.LastConnectedAddress ?? DefaultAddress;
        NameEditNode.Text = state.LastUsedName;

        CheckCanPressConnect();
    }

    private void CheckContent()
    {
        var state = RepoNode.GetAppState();
        if (state.LastUpdateDT is null)
        {
            OutdatedContentDialogNode.Show();
            return;
        }

        // TODO disable all distant match controls until confirmed that local content is not outdated
        ServerConnectionNode.RequestIsOutdated((DateTime)state.LastUpdateDT);
    }

    #region Signal connections

    public void OnConnectButtonPressed()
    {
        ServerConnectionNode.SetAddress(ServerAddressEditNode.Text);
        // TODO move this to ServerAddressEditNode
        var err = ConnectionRequestNode.Request($"{ServerAddressEditNode.Text}/api/v1/Home/Ping");
        // TODO check errs
        GD.Print(err);

        SetConnectionFormEditable(false);
    }

    private void SetConnectionFormEditable(bool v)
    {
        ServerAddressEditNode.Editable = v;
        NameEditNode.Editable = v;
        ConnectButtonNode.Disabled = !v;
    }

    public void OnServerAddressEditTextChanged(string _)
    {
        CheckCanPressConnect();
    }

    public void OnNameEditTextChanged(string _)
    {
        CheckCanPressConnect();
    }

    public void OnConnectionRequestRequestCompleted(HttpRequest.Result result, int responseCode, string[] headers, byte[] body)
    {
        if (result == HttpRequest.Result.CantConnect)
        {
            SetConnectionFormEditable(true);
            ConnectionErrorDialogNode.DialogText = "Failed to connect, server is likely offline";
            ConnectionErrorDialogNode.Show();
            return;
        }

        if (result != HttpRequest.Result.Success)
        {
            SetConnectionFormEditable(true);
            ConnectionErrorDialogNode.DialogText = $"Unrecognized response code: {responseCode}";
            ConnectionErrorDialogNode.Show();
            return;
        }

        // save name
        var appState = RepoNode.GetAppState();
        appState.LastUsedName = NameEditNode.Text;
        appState.LastConnectedAddress = ServerAddressEditNode.Text;
        RepoNode.UpdateAppState(appState);

        // show connection display
        ConnectionFormNode.Hide();
        ConnectionDisplayNode.Show();

        CheckContent();
    }

    public void OnContentCheckRequestRequestCompleted(HttpRequest.Result result, int responseCode, string[] headers, byte[] body)
    {
        GD.Print(responseCode);

        // var isOutdated = 
    }

    public void OnSyncContentButtonPressed()
    {
        ContentSyncWaitDialogNode.Show();
        ServerConnectionNode.RequestContentSynchronization();
    }
    
    public void OnServerConnectionNodeContentUpdateFinished()
    {
        var content = ServerConnectionNode.PopContentUpdate();
        RepoNode.ProcessContentUpdate(content);

        var state = RepoNode.GetAppState();
        state.LastUpdateDT = DateTime.Now.ToUniversalTime();
        RepoNode.UpdateAppState(state);

        ContentSyncWaitDialogNode.Hide();
        FinishedContentUpdateDialog.Show();

        // TODO emit signal, update content tab
    }

    public void OnServerConnectionNodeContentUpdateFailed(string errMsg)
    {
        // TODO display AcceptDialog
        GD.Print($"FAILED TO UPDATE CONTENT: {errMsg}");
    }

    #endregion
}
