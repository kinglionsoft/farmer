# 金丝雀升级

``` bash

deploy = "my-deploy"
image = "new-image"
imageName = "image-name"

# 新增一个pod
kubectl patch deployment $deploy -p '{"sepc": {"strategy": {"rollingUpdate": {"maxSurge": 1, "maxUnavailable": 0}}}}'

# 修改镜像tag
kubectl set image deloyment $deploy $imageName=$image \
&& kubectl rollout pause deployment $deploy

# 测试新版本

# 测试通过，全量升级
kubectl rollout resume deployment $deploy
kubectl rollout status deployment $deploy

# 若测试未通过，回滚
kubectl rollout undo deployment $deploy --to-revision=1

```