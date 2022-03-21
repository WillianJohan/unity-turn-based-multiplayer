using System.Collections;
using System.Collections.Generic;
using TMPro;
using WebSocketSharp;
using UnityEngine;
using System;
using System.Collections.Concurrent;

public class WebSocketClient : MonoBehaviour
{

    public TextMeshProUGUI TextBlock;
    public TextMeshProUGUI Response;


    #region WebSocket Variables/Components

    WebSocket webSocket;
    ConcurrentQueue<MessageFormat> incoming_messages = new ConcurrentQueue<MessageFormat>();
    enum MessageCode { OPEN, CLOSE, ERROR, MESSAGE };
    struct MessageFormat
    {
        public MessageCode CODE;
        public string Message;

        public MessageFormat(MessageCode code, string message)
        {
            this.CODE = code;
            this.Message = message;
        }

        public override string ToString()
        {
            return $"CODE:{CODE.ToString()} :: {Message}";
        }
    }

    #endregion

    private void Start()
    {
        Response.text = "";

        webSocket = new WebSocket("ws://localhost:8080");
        
        webSocket.OnMessage += HandleOnMessage;
        webSocket.OnOpen += HandleOnOpen;
        webSocket.OnClose += HandleOnClose;
        webSocket.OnError += HandleOnError;

        webSocket.Connect();
    }

    

    void LateUpdate()
    {
        if (!incoming_messages.TryDequeue(out var messagePackage))
            return;

        switch (messagePackage.CODE)
        {
            case MessageCode.OPEN:
                Debug.Log($"[OpennedConnection]::{messagePackage.Message}");
                break;
            case MessageCode.CLOSE:
                Debug.Log($"[ClosedConnection]::{messagePackage.Message}");
                break;
            case MessageCode.ERROR:
                Debug.Log($"[ERROR]::{messagePackage.Message}");
                break;
            case MessageCode.MESSAGE:
                Debug.Log($"[MessageReceived]::{messagePackage.Message}");
                UpdateResponse(messagePackage.Message);
                break;
            default:
                break;
        }

        Debug.Log("Queued: " + messagePackage);

    }


    public void SendMessage()
    {
        webSocket.Send(TextBlock.text);
    }

    void UpdateResponse(string message)
    {
        Response.text += "\n" + message;
    }


    #region WebSocket Event-Handlers


    private void HandleOnMessage(object sender, MessageEventArgs e)
        => incoming_messages.Enqueue(new MessageFormat(MessageCode.MESSAGE, e.Data));
    private void HandleOnOpen(object sender, EventArgs e)
        => incoming_messages.Enqueue(new MessageFormat(MessageCode.OPEN, "...ServerConnected"));
    private void HandleOnClose(object sender, CloseEventArgs e)
        => incoming_messages.Enqueue(new MessageFormat(MessageCode.CLOSE, e.Code.ToString()));
    private void HandleOnError(object sender, ErrorEventArgs e)
        => incoming_messages.Enqueue(new MessageFormat(MessageCode.ERROR, e.Message));


    #endregion

    

}
