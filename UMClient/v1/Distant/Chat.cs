using Godot;
using System;
using System.Threading.Tasks;
using UMDTO;

public partial class Chat : VBoxContainer
{
    #region Nodes

    [ExportGroup("Nodes")]
    [Export]
    public LineEdit MessageEditNode { get; set; }
    [Export]
    public Button SendButtonNode { get; set; }
    [Export]
    public RichTextLabel ChatDisplayNode { get; set; }

    #endregion

    private ServerConnection _connection;
    private string _matchId;

    public override void _Ready()
    {
        ChatDisplayNode.Clear();
    }


    public void SetEssentials(
        ServerConnection connection,
        string matchId
    )
    {
        _connection = connection;
        _matchId = matchId;

        _connection.ListenForChatUpdates(OnChatUpdate);
    }

    private void OnChatUpdate(ChatMessage msg)
    {
        Callable.From(() =>
        {
            var color = "red";
            var user = msg.From;
            if (msg.From == string.Empty)
            {
                color = "gray";
                user = "System";            
            }
            ChatDisplayNode.AppendText(
                $"[color={color}][{user}] {msg.Msg}[/color]\n"
            );
        }).CallDeferred();
    }

    private async Task Send()
    {
        if (!CheckCanSend())
        {
            return;
        }

        await _connection.PublishToChat(
            _matchId,
            MessageEditNode.Text
        );

        MessageEditNode.Clear();
    }

    private bool CheckCanSend()
    {
        var result = MessageEditNode.Text.Length > 0;
        SendButtonNode.Disabled = !result;
        return result;
    }

    #region Signal connections

    public void OnMessageEditTextChanged(string _)
    {
        CheckCanSend();
    }

    public async void OnMessageEditTextSubmitted(string _)
    {
        await Send();
    }

    public async void OnSendButtonPressed()
    {
        await Send();
    }

    #endregion
}
