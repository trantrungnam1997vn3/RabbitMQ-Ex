import React from "react";
import { NavLink } from "react-router-dom";
import { Nav, Site } from "tabler-react";
import "tabler-react/dist/Tabler.css";

const SiteNav = () => {
  return (
    <React.Fragment>
      <Nav.Item to="/" icon="globe" active value="Home"></Nav.Item>
      <Nav.Item to="/chart" icon="globe" value="RabbitMQ"></Nav.Item>
      <Nav.Item icon="globe" value="Other"></Nav.Item>
    </React.Fragment>
  );
};

const infoLink = [
  <NavLink to="/" activeClassName="selected">
    Link
  </NavLink>,
  <NavLink to="/" activeClassName="selected">
    Link
  </NavLink>,
  <NavLink to="/" activeClassName="selected">
    Link
  </NavLink>,
  <NavLink to="/" activeClassName="selected">
    Link
  </NavLink>,
  <NavLink to="/" activeClassName="selected">
    Link
  </NavLink>,
  <NavLink to="/" activeClassName="selected">
    Link
  </NavLink>,
  <NavLink to="/" activeClassName="selected">
    Link
  </NavLink>,
  <NavLink to="/" activeClassName="selected">
    Link
  </NavLink>
];

let notificationsObjects = {
  notify: [
    {
      avatarURL: "/assests/images/25.jpg",
      message: (
        <React.Fragment>
          <strong>Nathan</strong> pushed new commit: Fix page load performance
          issue.
        </React.Fragment>
      ),
      time: "10 minutes ago"
    },
    {
      avatarURL: "/assests/images/25.jpg",
      message: (
        <React.Fragment>
          <strong>Alice</strong> started new task: Tabler UI design.
        </React.Fragment>
      ),
      time: "1 hour ago"
    },
    {
      avatarURL: "/assests/images/25.jpg",
      message: (
        <React.Fragment>
          <strong>Rose</strong> deployed new version of NodeJS REST Api // V3
        </React.Fragment>
      ),
      time: "2 hours ago"
    }
  ]
};

function BaseLayout(props) {
  return (
    <Site.Wrapper
      headerProps={{
        href: "/",

        imageURL: "/assests/images/logo-ban-tay.jpg",
        itemsObjects: <SiteNav />,
        notificationsTray: {
          notificationsObjects: notificationsObjects.notify
        },
        accountDropdown: {
          avatarURL: "/assests/images/25.jpg",
          name: "Jane Pearson",
          description: "Administrator",
          options: [
            "profile",
            { icon: "settings", value: "Settings", to: "/settings" },
            "mail",
            "message",
            "divider",
            "help",
            "logout"
          ]
        }
      }}
      navProps={{ items: <SiteNav /> }}
      children={props.children}
      footerProps={{
        nav: "",
        note: "Hello",
        copyright: "Copyright by Me ",
        links: infoLink
      }}
    ></Site.Wrapper>
  );
}

export default BaseLayout;
