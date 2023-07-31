mkdir -p /opt/wg/
cp -u WG.Worker /opt/wg/worker
cp -n appsettings.json /opt/wg/appsettings.json
chmod +x /opt/wg/worker
cp wgworker.service /etc/systemd/system/
systemctl daemon-reload
systemctl start wgworker.service
systemctl status wgworker.service