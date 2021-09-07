# Ingress Caching

``` yaml
apiVersion: extensions/v1beta1
kind: Ingress
metadata:
  name: pi
  annotations:
    nginx.ingress.kubernetes.io/proxy-buffering: "on"
    nginx.ingress.kubernetes.io/proxy-buffer-size: "8k"
    nginx.ingress.kubernetes.io/proxy-max-temp-file-size: "1024m"
    nginx.ingress.kubernetes.io/configuration-snippet: |
      proxy_cache share-cache;
      proxy_cache_lock on;
      proxy_cache_valid any 30m;
      add_header X-Cache-Status $upstream_cache_status;
```