mkdir /usr/share/ca-certificates/yx.com
wget http://share.yx.com/registry.yx.com_ca_root.crt -O /usr/share/ca-certificates/yx.com/ca.crt
update-ca-certificates

cat << EOF > /etc/docker/daemon.json
{
    "insecure-registries" : ["registry.yx.com:443"]
}
EOF

mkdir -p /etc/systemd/system/docker.service.d
cat << EOF > /etc/systemd/system/docker.service.d/http-proxy.conf
[Service]
Environment="HTTPS_PROXY=http://127.0.0.1:8123" "NO_PROXY=localhost,127.0.0.1,registry.docker-cn.com,registry.hub.docker.com,registry.yx.com"

EOF