import React, { useEffect, useRef, useState } from 'react';

const sso_login = async () => {
  let token = new URL(window.location.href).searchParams.get("sso_i");
  if (typeof token != "string" || token.length === 0) {
    return undefined;
  }
  let req = await fetch("/api/signin", {
    "method": "POST",
    "headers": {
      "Content-Type": "text/plain",
    },
    "body": token
  });
  if (!req.ok) {
    return undefined;
  }
  return await req.json();
};

const Home = () => {
  const [getAvatar, setAvatar] = useState();
  const [getName, setName] = useState();
  const called = useRef(false);

  const do_login = async () => {
    // every callback mode login token can be used only once.
    if (called.current) {
      return;
    }
    called.current = true;
    sso_login().then((result) => {
      if (typeof result != "object") {
        window.location.assign('/');
        return;
      }
      setName(result.n);
      setAvatar(result.a);
    });
  };

  useEffect(() => {
    do_login()
  }, []);

  return (<>
    <p>Welcome, {getName}</p>
    <br />
    <img src={getAvatar} alt='Avatar not found.' />
  </>);
};

export default Home;