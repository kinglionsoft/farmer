echo ""

echo "===================================================="

echo "Pull Istio 1.0.0 Images from aliyuncs.com ..."

echo "===================================================="

echo ""

echo "docker tag to openthings ..."

## 添加Tag for registry.cn-hangzhou.aliyuncs.com/openthings

MY_REGISTRY=registry.cn-hangzhou.aliyuncs.com/openthings
LOCAL_REGISTRY=192.168.0.240:5000

echo ""

echo "===================================================="

echo ""

## Pull镜像

echo ""

echo "1.istio-proxyv2:1.0.0"

docker pull ${MY_REGISTRY}/istio-proxyv2:1.0.0

docker tag ${MY_REGISTRY}/istio-proxyv2:1.0.0 gcr.io/istio-release/proxyv2:1.0.0

echo ""

echo "2.istio-proxy_init:1.0.0"

docker pull ${MY_REGISTRY}/istio-proxy_init:1.0.0

docker tag ${MY_REGISTRY}/istio-proxy_init:1.0.0  gcr.io/istio-release/proxy_init:1.0.0

echo ""

echo "3.istio-sidecar_injector:1.0.0"

docker pull ${MY_REGISTRY}/istio-sidecar_injector:1.0.0

docker tag ${MY_REGISTRY}/istio-sidecar_injector:1.0.0 gcr.io/istio-release/sidecar_injector:1.0.0

echo ""

echo "4.istio-galley:1.0.0"

docker pull ${MY_REGISTRY}/istio-galley:1.0.0

docker tag ${MY_REGISTRY}/istio-galley:1.0.0 gcr.io/istio-release/galley:1.0.0

echo ""

echo "5.istio-mixer:1.0.0"

docker pull ${MY_REGISTRY}/istio-mixer:1.0.0

docker tag ${MY_REGISTRY}/istio-mixer:1.0.0 gcr.io/istio-release/mixer:1.0.0

echo ""

echo "6.istio-pilot:1.0.0"

docker pull ${MY_REGISTRY}/istio-pilot:1.0.0

docker tag ${MY_REGISTRY}/istio-pilot:1.0.0 gcr.io/istio-release/pilot:1.0.0

echo ""

echo "7.istio/citadel:1.0.0"

docker pull ${MY_REGISTRY}/istio-citadel:1.0.0

docker tag ${MY_REGISTRY}/istio-citadel:1.0.0 gcr.io/istio-release/citadel:1.0.0

echo ""

echo "8.coreos-hyperkube:v1.7.6_coreos.0"

docker pull $LOCAL_REGISTRY/hyperkube:v1.7.6_coreos.0

docker tag $LOCAL_REGISTRY/hyperkube:v1.7.6_coreos.0 quay.io/coreos/hyperkube:v1.7.6_coreos.0

echo ""

echo "9.grafana:1.0.0"

docker pull $LOCAL_REGISTRY/grafana:1.0.0

docker tag $LOCAL_REGISTRY/grafana:1.0.0 gcr.io/istio-release/grafana:1.0.0

echo ""

echo "10.servicegraph:1.0.0"

docker pull $LOCAL_REGISTRY/servicegraph:1.0.0

docker tag $LOCAL_REGISTRY/servicegraph:1.0.0 gcr.io/istio-release/servicegraph:1.0.0

echo ""

echo "===================================================="

echo "Push Istio 1.0.0 Images FINISHED."

echo "into registry.cn-hangzhou.aliyuncs.com/openthings, "

echo "by openthings@https://my.oschina.net/u/2306127."

echo "===================================================="

echo ""