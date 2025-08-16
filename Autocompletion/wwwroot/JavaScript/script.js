let ws = new WebSocket("https://localhost:7128/ws");
let svg = d3.select("svg"),
    width = +svg.attr("width"),
    height = +svg.attr("height");

let g = svg.append("g").attr("transform", "translate(150,50)");

let treeLayout = d3.tree().size([height - 100, width - 400]);

document.getElementById("msg").addEventListener("input", () => {
    let msg = document.getElementById("msg").value;
    if (ws.readyState === WebSocket.OPEN) {
        ws.send(msg);
    }
});

ws.onmessage = event => {
    let data = JSON.parse(event.data);
    console.log("event.data: ", event.data);

    updateSuggestions(data);
    let temp = wordsToTree(data);
    renderTrie(temp);
};

function updateSuggestions(words) {
    let ul = document.getElementById("suggestions");
    ul.innerHTML = "";

    words.forEach(word => {
        let li = document.createElement("li");
        li.textContent = word;
        ul.appendChild(li);
    });
}

function wordsToTree(words) {
    let temp = document.getElementById("msg").value;
    let root = { letter: temp, children: [] };

    for (let word of words) {
        root.children.push({
            letter: word,
            children: []
        });
    }
    return root;
}

function renderTrie(data) {
    g.selectAll("*").remove();

    let root = d3.hierarchy(data, d => d.children);
    treeLayout(root);

    g.selectAll(".link")
        .data(root.links())
        .enter().append("path")
        .attr("class", "link")
        .attr("d", d3.linkHorizontal()
            .x(d => d.y)
            .y(d => d.x)
        );

    let node = g.selectAll(".node")
        .data(root.descendants())
        .enter().append("g")
        .attr("class", "node")
        .attr("transform", d => `translate(${d.y},${d.x})`);

    node.append("circle")
        .attr("r", 10);

    node.append("text")
        .attr("dy", 3)
        .attr("x", d => d.children ? -12 : 12)
        .style("text-anchor", d => d.children ? "end" : "start")
        .text(d => d.data.letter ? d.data.letter : "root");
}

function sendMessage() {
    let msg = document.getElementById("msg").value;
    ws.send(msg);
}