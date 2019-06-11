# Linkerd 部署
Linkerd 支持GRPC负载均衡

## Features
https://linkerd.io/2/features/
* HTTP, HTTP/2, and gRPC Proxying
* 暂时不支持双向流请求的负载均衡
## Setup
https://linkerd.io/2/getting-started/

* 使用本地docker镜像

```bash
linkerd install | ./local-img-inject.sh pull -

linkerd install | ./local-img-inject.sh inject - | kubectl apply -f -

docker pull registry.yx.com/gcr.io/linkerd-io/controller:stable-2.3.1 && docker tag registry.yx.com/gcr.io/linkerd-io/controller:stable-2.3.1 gcr.io/linkerd-io/controller:stable-2.3.1
docker pull registry.yx.com/gcr.io/linkerd-io/proxy:stable-2.3.1 && docker tag registry.yx.com/gcr.io/linkerd-io/proxy:stable-2.3.1 gcr.io/linkerd-io/proxy:stable-2.3.1
docker pull registry.yx.com/gcr.io/linkerd-io/proxy-init:stable-2.3.1 && docker tag registry.yx.com/gcr.io/linkerd-io/proxy-init:stable-2.3.1 gcr.io/linkerd-io/proxy-init:stable-2.3.1
docker pull registry.yx.com/gcr.io/linkerd-io/web:stable-2.3.1 && docker tag registry.yx.com/gcr.io/linkerd-io/web:stable-2.3.1 gcr.io/linkerd-io/web:stable-2.3.1
docker pull registry.yx.com/prom/prometheus:v2.7.1 && docker tag registry.yx.com/prom/prometheus:v2.7.1 prom/prometheus:v2.7.1
docker pull registry.yx.com/gcr.io/linkerd-io/grafana:stable-2.3.1 && docker tag registry.yx.com/gcr.io/linkerd-io/grafana:stable-2.3.1 gcr.io/linkerd-io/grafana:stable-2.3.1


```

* Exposing the Dashboard
https://linkerd.io/2/tasks/exposing-dashboard/#traefik
```bash
# local port
linkerd dashboard

# expose
kubectl port-forward service/linkerd-web -n linkerd --address=0.0.0.0 8084:8084

```

* Injection

``` bash
linkerd inject deployment.yml  | kubectl apply -f -
```

* Automating Injection
https://linkerd.io/2/tasks/automating-injection/

``` bash
linkerd install --proxy-auto-inject | kubectl apply -f -
```

```yaml
apiVersion: extensions/v1beta1
kind: Deployment
metadata:
  name: helloworld-enabled
  labels:
    run: helloworld-enabled
spec:
  replicas: 1
  selector:
    matchLabels:
      run: helloworld-enabled
  template:
    metadata:
      annotations:
        linkerd.io/inject: enabled  # !!!!! here
      labels:
        run: helloworld-enabled
    spec:
      containers:
      - name: helloworld-enabled
        image: buoyantio/helloworld
```
## Configuration

### GRPC

https://kubernetes.io/blog/2018/11/07/grpc-load-balancing-on-kubernetes-without-tears/?tdsourcetag=s_pctim_aiomsg