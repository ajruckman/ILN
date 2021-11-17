dotnet clean

Remove-Item -Force -Recurse Plugins/*

dotnet publish -c Release -f net6.0 ILN.GRPC.Service.Plugin.SplunkForwarder

Copy-Item -Recurse ILN.GRPC.Service.Plugin.SplunkForwarder\bin\Release\net6.0\publish/ Plugins/ILN.GRPC.Service.Plugin.SplunkForwarder
