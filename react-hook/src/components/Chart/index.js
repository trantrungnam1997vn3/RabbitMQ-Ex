import React, { useState, useEffect } from "react";
import {
  Page,
  Notification,
  Form,
  Container,
  Card,
  Button
} from "tabler-react";
import { stat } from "fs";

function Chart() {
  const [message, setMessage] = useState("");

  useEffect(() => {
    console.log(message);
  });

  return (
    <Page.Content>
      <Container>
        <Card>
          <Card.Header>
            <Card.Title>Chat with BOT</Card.Title>
          </Card.Header>
          <Card.Body>
            <div className="d-flex">
              <Notification
                avatarURL="demo/faces/female/1.jpg"
                message={
                  <React.Fragment>
                    <strong>Alice</strong> Hello Nam
                  </React.Fragment>
                }
                time="1 hour ago"
                unread={false}
              />
            </div>
          </Card.Body>
          <Card.Footer>
            <Card.Options>
              <Form.InputGroup append={<Button color="primary">SEND</Button>}>
                <Form.Input
                  onChange={e => setMessage(e.target.value)}
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
