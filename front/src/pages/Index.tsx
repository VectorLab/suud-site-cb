import React from 'react';
/**
 * Client side only have permission to access site_id
 * DO NOT allow client side access key_pri or key_pwd
 */
const ssoClientID = "66f942e782ab48e331fb313a"; // your sso site id here

const oauthLogin = () => {
  const SUUD = new URL("/sso", "https://suud.net");
  SUUD.searchParams.set("s", ssoClientID);
  SUUD.searchParams.set("t", "cb");

  // Set permission parameters:
  // 1: user ID
  // 2: username
  // 3: avatar
  const permissions = [1, 2, 3];
  permissions.forEach(p => SUUD.searchParams.append("p", p.toString()));

  const callbackURL = new URL("/home", window.location.href);
  SUUD.searchParams.set("cb", callbackURL.toString());

  try {
    /**
     * DO NOT use window.location.replace,
     * which will cause current site unable to handle error
     */
    window.location.assign(SUUD.toString());
  } catch (error) {
    console.error(error);
  }
};

const Index = () => {
  return (
    <button className="btn btn-primary" onClick={oauthLogin}>enter</button>
  );
};

export default Index;