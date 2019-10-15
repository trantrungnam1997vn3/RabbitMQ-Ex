import React from "react";
import { Site } from "tabler-react";

const Copyright = () => {
    return (
        <p>Copyright by Nam</p>
    )
}

const FooterNavbar = () => {
    return (
        <p>Foo</p>
    )
}

function Footer() {

    return (
        <Site.Footer copyright={Copyright} links="Home" nav={FooterNavbar} note="abcd">

        </Site.Footer>
    )
}

export default Footer;