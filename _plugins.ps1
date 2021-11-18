dotnet clean ILN.GRPC.Service.Plugin.SplunkForwarder

Remove-Item -Force -Recurse Plugins/*

dotnet publish -c Debug -f net6.0 ILN.GRPC.Service.Plugin.SplunkForwarder

Copy-Item -Recurse ILN.GRPC.Service.Plugin.SplunkForwarder\bin\Debug\net6.0\publish/ Plugins/ILN.GRPC.Service.Plugin.SplunkForwarder
