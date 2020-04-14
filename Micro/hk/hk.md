# 香港节点

### iptables转发

``` bash
systemctl stop firewalld
yum install iptables-services
systemctl enable iptables

vim /etc/sysctl.conf
net.ipv4.ip_forward = 1

sysctl -p
# hk <--> ctc-web 47.244.4.145:172.31.15.228
iptables -t nat -A PREROUTING -m tcp -p tcp --dport 80 -j DNAT --to 120.78.245.159
iptables -t nat -A PREROUTING -m tcp -p tcp --dport 443 -j DNAT --to 120.78.245.159
iptables -t nat -I POSTROUTING -p tcp --dport 80 -j MASQUERADE
iptables -t nat -I POSTROUTING -p tcp --dport 443 -j MASQUERADE

# hk <--> ctc-client $ update, 47.240.18.147:172.31.15.231
iptables -t nat -A PREROUTING -m tcp -p tcp --dport 80 -j DNAT --to 47.112.85.138
iptables -t nat -A PREROUTING -m tcp -p tcp --dport 443 -j DNAT --to 47.106.15.115
iptables -t nat -I POSTROUTING -p tcp --dport 80 -j MASQUERADE
iptables -t nat -I POSTROUTING -p tcp --dport 443 -j MASQUERADE

# hk <--> aws k8s 
iptables -t nat -A PREROUTING -m tcp -p tcp --dport 6443 -j DNAT --to 15.165.129.212:80
iptables -t nat -I POSTROUTING -p tcp --dport 6443 -j MASQUERADE

# save
service iptables save
service iptables restart

# list
iptables -L -t nat

# delete
iptables -L -t nat --line-numbers
iptables -t nat -D PREROUTING 1
``` 

