# Netplan

## 多网卡

``` yaml
# /etc/netplan/50-cloud-init.yaml
network:
    ethernets:
        ens160:
            addresses:
            - 192.168.2.24/24
            gateway4: 192.168.2.1
            nameservers:
                addresses:
                - 192.168.2.21
            routes:
                - to: 0.0.0.0/0
                  via: 192.168.1.1
                  metric: 100
        ens192:
            addresses:
            - 192.168.1.0/24
            gateway4: 192.168.1.1
            nameservers:
                addresses:
                - 192.168.1.24
            routes:
                - to: 192.168.1.0/24
                  via: 192.168.1.1
                  metric: 99

    version: 2

```