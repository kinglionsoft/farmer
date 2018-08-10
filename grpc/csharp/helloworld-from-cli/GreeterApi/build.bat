dotnet publish -c Release -o publish 
docker build -t 192.168.0.240:5000/greeter_api .
docker push 192.168.0.240:5000/greeter_api


