sudo mkdir /usr/local/share/ca-certificates/local.com
sudo wget http://share.local.com/registry.local.com_ca_root.crt -O /usr/local/share/ca-certificates/local.com/ca.crt
sudo update-ca-certificates

sudo mkdir -p /etc/docker/certs.d/registry.local.com:443
sudo cp /usr/local/share/ca-certificates/local.com/ca.crt /etc/docker/certs.d/registry.local.com:443/ca.crt