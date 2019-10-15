import React from "react";
import { Site, AccountDropdown } from "tabler-react";

const accountDropdown = () => {
  return (
    <AccountDropdown
      avatarURL="./demo/faces/female/25.jpg"
      name="Jane Pearson"
      description="Administrator"
      options={[
        "profile",
        { icon: "settings", value: "Settings", to: "/settings" },
        "mail",
        "message",
        "divider",
        "help",
        "logout"
      ]}
    />
  );
};

function HeaderNav(props) {
  return (
    <Site.Header
      {...props}
      href="/home"
      imageURL="/src/assests/images/25.jpg"
      alt="Im Here"
      abcd="dcdc"
      accountDropdown={<accountDropdown abcd="asd" />}
    ></Site.Header>
  );
}

export default HeaderNav;
