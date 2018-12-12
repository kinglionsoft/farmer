# 本地IDC

## 配置

* 创建主机（eson-entry, 双网卡：172.16.0.20，192.168.0.54）；
* 在H3C路由器上，开启防火墙，将80、443端口指向172.16.0.20；
* eson-entry 作为节点加入k8s集群，原则上但不承载Pod，只作为入口；

``` bash
kubectl drain eson-entry
```