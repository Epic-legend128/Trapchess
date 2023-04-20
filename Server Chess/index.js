//
//  Server w/ js 
//  Edoardo, Fotios, Gurnaaz
//

// GLOBAL CONSTANTS

const express = require('express');
const app = express();
const port = 3000;
const http = require('http');
const server = http.createServer(app)

// SOCKET IO integr.
const { SERVER } = require('socket.io')
const io = new SERVER(server)

app.get("/", (req, res) => {
    res.send('<h1> hello world </h1>');
})
try {
    app.listen(port, () => {
        console.log(`[STARTED] Running on port ${port}`);
    })
}
catch (err) {
    console.log(`[ERROR] -> ${err.name + " :   " + err.message}`);
}