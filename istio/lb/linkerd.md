# Linkerd 部署
Linkerd 支持GRPC负载均衡

## Features
https://linkerd.io/2/features/
* HTTP, HTTP/2, and gRPC Proxying
## Setup
https://linkerd.io/2/getting-started/

* 使用本地docker镜像

```bash
linkerd install | ./local-img-inject.sh pull -

linkerd install | ./local-img-inject.sh inject - | kubectl apply -f -

```

* Exposing the Dashboard
https://linkerd.io/2/tasks/exposing-dashboard/#traefik
```bash
# local port
linkerd dashboard

# expose
kubectl port-forward service/linkerd-web -n linkerd --address=0.0.0.0 8084:8084

```

* Automating Injection
https://linkerd.io/2/tasks/automating-injection/

``` bash
linkerd install --proxy-auto-inject | kubectl apply -f -
```
## Configuration

### GRPC

https://kubernetes.io/blog/2018/11/07/grpc-load-balancing-on-kubernetes-without-tears/?tdsourcetag=s_pctim_aiomsg