# 阿里云

## Pod不能访问公网
阿里云的istio默认拦截了外网

``` bash
kubectl get configmap istio -n istio-system -o yaml | sed 's/mode: REGISTRY_ONLY/mode: ALLOW_ANY/g' | kubectl replace -n istio-system -f -

```