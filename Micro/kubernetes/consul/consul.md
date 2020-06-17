# Consul

## 安装
[https://www.consul.io/docs/platform/k8s/run.html](https://www.consul.io/docs/platform/k8s/run.html)

* Helm没有创建持久化卷，需要手动创建

``` bash

tee data-consul-0-pv.yaml << EOF
kind: PersistentVolume
apiVersion: v1
metadata:
  name: data-default-test-consul-server-0
  labels:
    type: local
spec:
  storageClassName: manual
  capacity:
    storage: 10Gi
  accessModes:
    - ReadWriteOnce
  hostPath:
    path: "/data/consul/data0"

---    
kind: PersistentVolumeClaim
apiVersion: v1
metadata:
  name: data-default-test-consul-server-0
spec:
  storageClassName: manual
  accessModes:
    - ReadWriteOnce
  resources:
    requests:
      storage: 10Gi
---
kind: PersistentVolume
apiVersion: v1
metadata:
  name: data-default-test-consul-server-1
  labels:
    type: local
spec:
  storageClassName: manual
  capacity:
    storage: 10Gi
  accessModes:
    - ReadWriteOnce
  hostPath:
    path: "/data/consul/data1"

---    
kind: PersistentVolumeClaim
apiVersion: v1
metadata:
  name: data-default-test-consul-server-1
spec:
  storageClassName: manual
  accessModes:
    - ReadWriteOnce
  resources:
    requests:
      storage: 10Gi
EOF

kubectl create -f data-consul-0-pv.yaml

kubectl port-forward consul-consul-server-0 --address=0.0.0.0 8500:8500
```

## Consul Gateway
配置Istio Ingress gateway 允许集群外访问配置界面

``` bash

# istio inject

kubectl get statefulset consul-consul-server -o yaml | istioctl kube-inject -f - | kubectl apply -f -

# gateway
kubectl apply -f - <<EOF
apiVersion: networking.istio.io/v1alpha3
kind: Gateway
metadata:
  name: consul-gateway
spec:
  selector:
    istio: ingressgateway
  servers:
  - port:
      number: 80
      name: http
      protocol: HTTP
    hosts:
    - "consul.local.com"
---
apiVersion: networking.istio.io/v1alpha3
kind: VirtualService
metadata:
  name: consul
spec:
  hosts:
  - "consul.local.com"
  gateways:
  - consul-gateway
  http:
  - match:
    - uri:
        prefix: /
    route:
    - destination:
        port:
          number: 80
        host: consul-consul-ui
EOF

# 访问地址: http://consul.local.com

```

## 访问Consul Agent

* 每个节点上都以DaemonSet的方式运行了一个Consul Agent。
* 节点上的服务直接通过本地的8500端口连接agent。常用的获取节点Host IP的方式是[Downward API](https://kubernetes.io/docs/tasks/inject-data-application/environment-variable-expose-pod-information/#the-downward-api)

``` yaml
containers:
  - name: ocelot
    imagePullPolicy: IfNotPresent
    image: registry.local.com/test/ocelot:2.0.0
    env: 
      - name: GlobalConfiguration__ServiceDiscoveryProvider__Host
        valueFrom:
          fieldRef:
            fieldPath: status.hostIP
```


## KV备份

``` bash
# proxy
kubectl port-forward test-consul-server-0 --address=0.0.0.0 8500

# backup
.\consul.exe kv export -http-addr=http://consul.local.com | out-file -encoding utf8 test-consul-bak.json

# import 注意编码改为utf8
.\consul.exe kv import -http-addr=http://192.168.0.249:8500 @test-consul-bak.json
```

## 阿里云

* DaemonSet的hostPort不起作用：https://github.com/hashicorp/consul-helm/pull/194
* 使用Headless Service访问consul-agent
```yaml
apiVersion: v1
kind: Service
metadata:
  name: consul-agent
  labels:
    app: consul-agent
spec:
  ports:
  - port: 8500
    targetPort: 8500
    nodePort: 30500
    name: http  
  selector:
    app: consul
    component: client
  type: NodePort
```