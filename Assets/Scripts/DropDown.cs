using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class DropDown : MonoBehaviour
{
    private Dropdown dropdown;
    private HelloClient helloClient;

    private async Task Start()
    {
        dropdown = GetComponent<Dropdown>();
        helloClient = new HelloClient();
        await OptionList();
    }

    private async Task OptionList ()
    {
        foreach (var type in Enum.GetValues(typeof(HelloStreamRpcType)))
        {
            dropdown.options.Add(new Dropdown.OptionData() { text = type.ToString() });
        }

        await OnValueChanged((int)HelloStreamRpcType.Unary);
        dropdown.onValueChanged.AddListener(async (value) => await OnValueChanged(value));

       //dropdown.RefreshShownValue();
    }

    public async Task OnValueChanged (int value)
    {
        //Debug.Log($"{value}”Ô–Ú‚Ì—v‘f");
        switch (value)
        {
            case (int)HelloStreamRpcType.Unary:
                helloClient.Hello();
                break;
            case (int)HelloStreamRpcType.ServerStreaming:
                await helloClient.HelloServerStream();
                break;
            case (int)HelloStreamRpcType.ClientStreaming:
                await helloClient.HelloClientStream();
                break;
            case (int)HelloStreamRpcType.BidirectionalStreaming:
                await helloClient.HelloBiStreams();
                break;
        }
    }
}
