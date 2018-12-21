# 本地IDC

## 配置

* 创建主机（eson-entry, 双网卡：172.16.0.20，192.168.0.54）；
* 在H3C路由器上，开启防火墙，将80、443端口指向172.16.0.20；
* eson-entry 作为节点加入k8s集群，原则上但不承载Pod，只作为入口；

``` bash
kubectl drain eson-entry
# kubectl uncordon  -- Make the node schedulable again:
```

## 使用浏览器访问 Ingress 服务
通过修改 Ingress Gateway 的 Deployment，将 80 端口和 443 端口配置为 hostPort 模式，然后再通过 Node 亲和性将 Gateway 调度到某个固定的主机上。

``` bash
kubectl -n istio-system edit deployment istio-ingressgateway
```
``` yaml
apiVersion: extensions/v1beta1
kind: Deployment
metadata:
  name: istio-ingressgateway
  namespace: istio-system
  ...
spec:
  ...
  template:
    ...
    spec:
      affinity:
        nodeAffinity:
          ...
          requiredDuringSchedulingIgnoredDuringExecution:
            nodeSelectorTerms:
            - matchExpressions:
              - key: kubernetes.io/hostname
                operator: In
                values:
                - eson-entry   # 比如你想调度到这台主机上
      containers:
        - name: ISTIO_META_POD_NAME
        ...
        - containerPort: 80
          hostPort: 80 # 绑定主机80端口
          protocol: TCP
        - containerPort: 443
          hostPort: 443 # 绑定主机443端口
          protocol: TCP
        ...
```
## HTTPS

```bash
kubectl create -n istio-system secret tls istio-ingressgateway-certs --key /home/k8s/yitu666.key --cert /home/k8s/yitu666.crt
```

## 配置文件存储服务代理

``` bash
tee file-http-gateway.yaml << EOF
apiVersion: networking.istio.io/v1alpha3
kind: Gateway
metadata:
  name: file-gateway
spec:
  selector:
    istio: ingressgateway # use Istio default gateway implementation
  servers:
  - port:
      number: 443
      name: https
      protocol: HTTPS
    tls:
      mode: SIMPLE
      serverCertificate: /etc/istio/ingressgateway-certs/tls.crt
      privateKey: /etc/istio/ingressgateway-certs/tls.key
    hosts:
    - "store.yitu666.com"
---
apiVersion: networking.istio.io/v1alpha3
kind: VirtualService
metadata:
  name: file-store
spec:
  hosts:
  - "store.yitu666.com"
  gateways:
  - file-gateway
  http:
  - match:
    - uri:
        prefix: /file
    route:
    - destination:
        port:
          number: 8082
        host: file-store
EOF
```
## iptables
iptables -N IN_WEB
iptables -I IN_WEB -j ACCEPT
iptables -I INPUT --dport 8443 -j IN_WEB
iptables -I OUTPUT --sport 8443 -j IN_WEB

iptables -A IN_WEB -p tcp --dport 8443 -j LOG --log-prefix "iptables:"

iptables -nvL IN_WEB


iptables -I INPUT -p tcp --dport 8443 --syn -j ACCEPT
iptables -I OUTPUT -p tcp --sport 8443 --syn -j ACCEPT

iptables -A OUTPUT -m state --state NEW -m tcp -p tcp --dport 8443 -j ACCEPT