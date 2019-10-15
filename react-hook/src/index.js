import React from "react";
import ReactDOM from "react-dom";
import { BrowserRouter, Route, Switch } from 'react-router-dom';
import App from "./App";
import Chart from "./components/Chart";
import "./index.css";
import * as serviceWorker from "./serviceWorker";
import BaseLayout from "./components/Site";
import "tabler-react/dist/Tabler.css";


function Index() {
  return (
    <BrowserRouter>
      <BaseLayout>
        <Switch>
          <Route exact path="/" component={App} />
          <Route exact path="/chart" component={Chart} />
          {/* <Route path="/women" component={Women} />
          <Route path="/men" component={Men} />
          <Route path="/clothes" component={Clothes} />
          <Route path="/accessories" component={Accessories} />
          <Route exact path="/products/:id" component={ShowProduct} /> */}
        </Switch>
      </BaseLayout>
    </BrowserRouter>
  );
}

ReactDOM.render(<Index />, document.getElementById("root"));

// If you want your app to work offline and load faster, you can change
// unregister() to register() below. Note this comes with some pitfalls.
// Learn more about service workers: https://bit.ly/CRA-PWA
serviceWorker.unregister();
