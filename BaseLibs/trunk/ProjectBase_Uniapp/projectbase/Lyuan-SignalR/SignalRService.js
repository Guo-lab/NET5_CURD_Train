import { HubConnection, HubConnectionBuilder, LogLevel } from "./signalr";

export const SignalRService= {
    HubConnectionBuilder: HubConnectionBuilder,
    LogLevel: LogLevel,
    HubConnectionState:HubConnectionState//目前signalr.js库不支持HubConnectionState，下面自己定义
};

export const ConnectionBuilder = HubConnectionBuilder

export const HubConnectionState = {
    Connected: 1,
    Reconnecting: 2,
    Reconnected:3,
    Closed:9
}
