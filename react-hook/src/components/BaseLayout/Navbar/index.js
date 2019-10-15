import React from "react";
import { NavLink } from "react-router-dom";

import "./index.css";

function NavBar() {
  return (
    <nav className="navbar">
      <div className="nav-links">
        <ul>
          <li>
            <NavLink
              activeClassName="selected"
              className="nav-link"
              exact
              to="/"
            >
              Home
            </NavLink>
          </li>
          <li>
            <NavLink
              activeClassName="selected"
              className="nav-link"
              to="/women"
            >
              Women
            </NavLink>
          </li>
          <li>
            <NavLink activeClassName="selected" className="nav-link" to="/men">
              Men
            </NavLink>
          </li>
        </ul>
      </div>
    </nav>
  );
}

export default NavBar;
