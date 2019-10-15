import React, { useState, useEffect } from "react";
import { NavLink } from "react-router-dom";

import { Site } from "tabler-react";

import Header from "./Header";
import Navbar from "./Navbar";
import Footer from "./Footer";
import "tabler-react/dist/Tabler.css";

function BaseLayout(props) {
  const [count, setCount] = useState(0);

  const classNames = ["first-header", "second-header", "thrid-header"];

  function handleClickCount(state) {
    setCount(state.count);
  }

  return (
    <div>
      <div>
        <Navbar />
        <Header />
      </div>
      <div className="content">{props.children}</div>
      <Footer />
    </div>
  );
}

export default BaseLayout;
