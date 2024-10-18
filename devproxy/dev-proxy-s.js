/**
 * This reserve proxy can be used for development environment only.
 * Use nginx for production environment instead.
 */
const https = require('https');
const httpProxy = require('http-proxy');
const fs = require('fs');
/**
 * suud.net will validate the jump back domain
 * Settings area
 */
const https_public_key = '/your/public_key.pem';
const https_private_key = '/your/private_key.pem';
const in_port = 443;

const target_ajax = 'http://localhost:5000';
const target_page = 'http://localhost:3000';

const proxy = httpProxy.createProxyServer({});

function isApiRequest(reqUrl) {
  return /^\/api($|\/)/.test(reqUrl);
};

const server = https.createServer({
  key: fs.readFileSync(https_private_key),
  cert: fs.readFileSync(https_public_key)
}, (req, res) => {
  if (isApiRequest(req.url)) {
    req.url = req.url.slice(4);
    if ('' == req.url) {
      req.url = '/';
    }
    proxy.web(req, res, {
      target: target_ajax
    });
  } else {
    proxy.web(req, res, {
      target: target_page
    });
  }
});

// Handle WebSocket connections
server.on('upgrade', (req, socket, head) => {
  if (isApiRequest(req.url)) {
    req.url = req.url.slice(4);
    if ('' == req.url) {
      req.url = '/';
    }
    proxy.ws(req, socket, head, {
      target: target_ajax
    });
  } else {
    proxy.ws(req, socket, head, {
      target: target_page
    });
  }
});

// Error handling
proxy.on('error', (err, req, res) => {
  console.error('Proxy error:', err, req.url);
  if (res.writeHead) {
    res.writeHead(500, { 'Content-Type': 'text/plain' });
    res.end('Proxy error');
  } else {
    console.error('Non-HTTP proxy error');
  }
});

server.listen(in_port, () => {
  console.log('HTTPS reverse proxy server listening on port ' + in_port);
});
