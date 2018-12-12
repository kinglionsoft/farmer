sudo mkdir /usr/local/share/ca-certificates/yx.com
sudo wget http://share.yx.com/registry.yx.com_ca_root.crt -O /usr/local/share/ca-certificates/yx.com/ca.crt
sudo update-ca-certificates

sudo mkdir -p /etc/docker/certs.d/registry.yx.com:443
sudo cp /usr/share/ca-certificates/yx.com/ca.crt /etc/docker/certs.d/registry.yx.com:443/ca.crt