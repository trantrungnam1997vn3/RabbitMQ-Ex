import React from "react";
import { Link} from "react-router-dom";

import "./index.css";

function NavBar() {
  return (
    <nav className="navbar">
      <div className="nav-links">
        <ul>
          <li>
            <Link
              activeClassName="selected"
              className="nav-link"
              exact
              to="/"
            >
              Home
            </Link>
          </li>
          <li>
            <Link
              activeClassName="selected"
              className="nav-link"
              to="/women"
            >
              Women
            </Link>
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
