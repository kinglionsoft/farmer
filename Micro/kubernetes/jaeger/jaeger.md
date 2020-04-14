# Jaeger on K8S

## 需求
下次升级到1.15.1
## 安装

https://github.com/jaegertracing/jaeger-kubernetes

``` bash
# prepare images
docker pull jaegertracing/jaeger-collector:1.15.1 \
  && docker tag jaegertracing/jaeger-collector:1.15.1 registry.local.com/mirror/jaeger-collector:1.15.1 \
  && docker push registry.local.com/mirror/jaeger-collector:1.15.1 \
docker pull jaegertracing/jaeger-query:1.15.1 \
  && docker tag jaegertracing/jaeger-query:1.15.1 registry.local.com/mirror/jaeger-query:1.15.1 \
  && docker push registry.local.com/mirror/jaeger-query:1.15.1 \
docker pull jaegertracing/jaeger-agent:1.15.1 \
  && docker tag jaegertracing/jaeger-agent:1.15.1 registry.local.com/mirror/jaeger-agent:1.15.1 \
  && docker push registry.local.com/mirror/jaeger-agent:1.15.1

# create
kubectl create -f jaeger.yaml

```

## 使用

### UI
http://k8s.local.com:26686/

http://ali202.k8s.local.com:30325

### 集群内访问Agent

#### 注入SideCar
Pod、Deployment上的服务通过SideCar的方式启动jaeger agent.

```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: myapp
spec:
  selector:
    matchLabels:
      app.kubernetes.io/name: myapp
  template:
    metadata:
      labels:
        app.kubernetes.io/name: myapp
    spec:
      containers:
      - image: mynamespace/hello-myimage
        name: myapp
        ports:
        - containerPort: 8080
      - image: registry.local.com/mirror/jaeger-agent:1.9.0  # sidecar 注入
        name: jaeger-agent
        ports:
        - containerPort: 5775
          protocol: UDP
        - containerPort: 6831
          protocol: UDP
        - containerPort: 6832
          protocol: UDP
        - containerPort: 5778
          protocol: TCP
        args: ["--collector.host-port=jaeger-collector:14267"]
```

#### 集群外访问Agent

``` yaml
apiVersion: v1
kind: Service
metadata:
  name: jaeger-agent-svc
  labels:
    app: jaeger
spec:
  ports:
  - name: jaeger-agent
    port: 6831
    protocol: UDP
    targetPort: 6831
    nodePort: 30831
  selector:
    app: client-service
  type: NodePort
---
apiVersion: extensions/v1beta1
kind: Deployment
metadata:
  labels:
    app: jaeger-agent
  name: jaeger-agent
spec:
  replicas: 1
  template:
    metadata:
      labels:
        app: jaeger-agent
      name: jaeger-agent
    spec:
      containers:
      - name: jaeger-agent
        image: registry.local.com/mirror/jaeger-agent:1.9.0  # jaeger sidecar
        ports:
        - containerPort: 5775
          protocol: UDP
        - containerPort: 6831
          protocol: UDP
        - containerPort: 6832
          protocol: UDP
        - containerPort: 5778
          protocol: TCP
        args: ["--reporter.type=tchannel", "--reporter.tchannel.host-port=jaeger-collector:14267"]
```

