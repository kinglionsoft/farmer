# Dapr

## Init

开发环境下，**dapr init**网络超时，需要开启代理：

``` bash
# 首先在IE浏览器中设置上网的代理
netsh winhttp import proxy source=ie
# 查看当前使用的代理
netsh winhttp show proxy
# 取消当前使用的代理
netsh winhttp rest proxy
```