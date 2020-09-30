# SSH

## 反向隧道

```bash

server_internet='47.99.39.24'
server_port='30111'
local_port='30111'

# on server
/etc/ssh/sshd_config
GatewayPorts yes
service sshd restart

# on lcoal
ssh -NfR $server_port:localhost:$local_port root@$server_internet
# or
autossh -M 40111 -fNR $server_port:localhost:$local_port root@$server_internet
autossh -M 40120 -fNR 30120:localhost:30120 root@$server_internet

```