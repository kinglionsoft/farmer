
ingress 在default命名空间

```yaml
kind: Service
apiVersion: v1
metadata:
  name: grafana
  namespace: default
spec:
  type: ExternalName
  externalName: grafana-service.monitor.svc.cluster.local
  ports:
  - port: 3000
```