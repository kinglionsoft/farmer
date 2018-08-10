dotnet publish -c Release -o publish -f netcoreapp2.1
docker build -t 192.168.0.240:5000/greeter_server .
docker push 192.168.0.240:5000/greeter_server


