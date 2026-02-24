using Godot;
using System;

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
        ServerConnectionNode.ContentOutdatedResponded += OnServerConnectionNodeContentOutdatedResponded;

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

    public async void OnConnectButtonPressed()
    {
        SetConnectionFormEditable(false);

        var registrationError = await ServerConnectionNode.Connect(
            ServerAddressEditNode.Text,
            NameEditNode.Text
        );
        if (!string.IsNullOrEmpty(registrationError))
        {
            SetConnectionFormEditable(false);
            ConnectionErrorDialogNode.DialogText = $"Failed to connect!\n{registrationError}";
            ConnectionErrorDialogNode.Show();
            return;
        }
        
        SetConnectionFormEditable(true);

        // save name
        var appState = RepoNode.GetAppState();
        appState.LastUsedName = NameEditNode.Text;
        appState.LastConnectedAddress = ServerAddressEditNode.Text;
        RepoNode.UpdateAppState(appState);

        // show connection display
        ConnectionFormNode.Hide();
        ConnectionDisplayNode.Show();

        // load content for match creation
        var configs = await ServerConnectionNode.FetchConfigs();
        GD.Print(configs.Count);
        var loadouts = await ServerConnectionNode.FetchLoadouts();
        GD.Print(loadouts.Count);
        

        CheckContent();


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

    public void OnServerConnectionNodeContentOutdatedResponded(bool isOutdated)
    {
        if (!isOutdated) return;

        OutdatedContentDialogNode.Show();
    }

    #endregion
}
