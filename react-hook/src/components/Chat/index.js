import React, { useState, useEffect } from "react";
import ReactDOM from "react-dom";
import Stomp from "stompjs";
import {
  Button,
  Card,
  Container,
  Form,
  Notification,
  Page
} from "tabler-react";

function Chart() {
  const [message, setMessage] = useState("");
  const [client, setClient] = useState(
    Stomp.client("ws://10.0.1.222:15674/ws")
  );
  const [queueName, setQueueName] = useState("");
  const [correlated_id, setCorrelatedId] = useState("");
  const [messageUser, setMessageUser] = useState([]);

  useEffect(() => {

    handleGetCorIdAndQueueName();
    createConnectionToWebSocket();
  }, [client]);

  function createConnectionToWebSocket() {
    client.connect("admin", "admin", on_connect, on_error, "/");
  }

  function handleGetCorIdAndQueueName() {
    return fetch("http://localhost:5000/api/message", {
      method: "GET"
    })
      .then(res => res.json())
      .then(
        result => {
          setQueueName(result.name);
          setCorrelatedId(result.id);
        },
        error => {
          console.log(error);
        }
      );
  }

  function on_connect() {
    client.subscribe(
      // "/reply-queue/" + queueName,

       "/exchange/direct_logs/amq.gen-0GwdxlIydlUjYJEWEU9n0Q",
      // "/reply-queue/rpc_queue",
      function(b) {
        setMessageUser(messageUser => {
          messageUser.push([b.body]);
          return messageUser;
        });
        var tag = document.getElementById("message");
        var child = document.createElement("div");
        tag.append(child);
        ShowMessage("BOT", b.body, false);
      },
      {
        // "correlation-id": correlated_id
        "correlation-id": "04b3a1d1-903a-4efa-870a-863b80aefc78" 
      }
    );

    setClient(client);
  }

  function handleSendMessage() {
    var headers = new Headers();
    headers.append("Content-Type", "application/json");
    headers.append("Accept", "application/json");
    headers.append("Access-Control-Allow-Credentials", "true");
    headers.append("Access-Control-Allow-Origin", "http://localhost:5000");

    headers.append("GET", "POST", "OPTIONS");

    fetch("http://localhost:5000/api/message", {
      method: "POST",
      mode: "cors",
      // credentials: "include",
      headers: {
        "Access-Control-Allow-Credentials": "true",
        "Access-Control-Allow-Origin": "http://localhost:3000",
        "Content-Type": "application/json"
      },
      body: JSON.stringify({
        reqdata: {
          Message: message
        }
      })
    })
      .then(res => res.json())
      .then(
        result => {
          console.log(result);
          // setQueueName(result.name);
          // queueNameVariable = result.name;
          // correlated_idVariable = result.id;
          // setCorrelatedId(result.id);
          // createConnectionToWebSocket();
          ShowMessage("NAM", message, true);
        },
        error => {
          console.log(error);
        }
      );
  }

  function ShowMessage(user, message, isSend) {
    var tag = document.getElementById("message");
    var child = document.createElement("div");
    tag.append(child);
    ReactDOM.render(
      <div className={`${isSend ? "d-flex flex-row-reverse" : "d-flex "}`}>
        <div className={`col-5 ${isSend ? "d-flex " : "d-flex"}`}>
          <Notification
            message={
              <React.Fragment>
                <p className="text-justify">
                  <strong>{user.toUpperCase()}: </strong> {message}{" "}
                </p>
              </React.Fragment>
            }
            time="1 hour ago"
            unread={false}
          />
        </div>
      </div>,
      child
    );
  }

  // function handleSendMessage() {
  //   console.log("go");
  //   client.send(
  //     "/exchange/topic_logs/abc",
  //     {
  //       "content-type": "text/plain",
  //       "correlation-id": correlated_id,
  //       "reply-to": "rpc_queue"
  //     },
  //     message
  //   );
  //   ShowMessage("NAM", message, true);
  // }

  function on_error() {
    console.log("error");
  }

  return (
    <Page.Content>
      <Container>
        <Card>
          <Card.Header>
            <Card.Title>Chat with BOT</Card.Title>
          </Card.Header>
          <Card.Body>
            <div id="message" />
            {/* {document.removeChild()} */}
            {/* {messageUser.map((item, id) => (
              <div className="d-flex" key={id}>
                <Notification
                  key={id}
                  avatarURL="demo/faces/female/1.jpg"
                  message={
                    <React.Fragment>
                      <strong>BOT</strong> {item}
                    </React.Fragment>
                  }
                  time="1 hour ago"
                  unread={false}
                />
              </div>
            ))} */}
          </Card.Body>
          <Card.Footer>
            <Card.Options>
              <Form.InputGroup
                append={
                  <Button onClick={e => handleSendMessage()} color="primary">
                    SEND
                  </Button>
                }
              >
                <Form.Input
                  onChange={e => {
                    setMessage(e.target.value);
                  }}
                  value={message}
                  placeholder="Input text"
                />
              </Form.InputGroup>
            </Card.Options>
          </Card.Footer>
        </Card>
      </Container>
    </Page.Content>
  );
}

export default Chart;
