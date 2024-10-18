import React, { useEffect, useRef, useState } from 'react';

const sso_login = async () => {
  let v2 = new URL(window.location.href).searchParams.get("sso_i");
  if (typeof v2 != "string" || v2.length === 0) {
    return undefined;
  }
  let v3 = await fetch("/api/signin", {
    "method": "POST",
    "headers": {
      "Content-Type": "text/plain",
    },
    "body": v2
  });
  if (!v3.ok) {
    return undefined;
  }
  return await v3.json();
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
    sso_login().then((v1) => {
      if (typeof v1 != "object") {
        window.location.assign('/');
        return;
      }
      setName(v1.n);
      setAvatar(v1.a);
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