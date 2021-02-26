```bash
kubectl create -f - <<EOF
apiVersion: dapr.io/v1alpha1
kind: Component
metadata:
  name: statestore
  namespace: default
spec:
  type: state.redis
  version: v1
  metadata:
  - name: redisHost
    value: redis.default.svc.cluster.local:6379
  - name: redisPassword
    value: ""
EOF
```