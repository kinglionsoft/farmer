# Prometheus + Grafana

## Install

https://www.jianshu.com/p/ac8853927528

### AWS
使用EFS作为持久化盘

### Aliyun
自建NFS作为持久化盘

#### NFS Server

* 172.18.181.176/119.23.222.242 - CentOS

``` bash
yum -y install nfs-utils

# start rpcbind
systemctl start rpcbind

# /etc/exports
/data    172.18.0.0/16(rw,async)

# start nfs
systemctl start nfs 

# /data access
chown -R nfsnobody.nfsnobody /data
```
