export class Transport{
    constructor(id, type){
        this.id = id
        this.type = type
    }

    DrawTransport(host){
        var singleTransport = document.createElement("input")
        singleTransport.type = "radio"
        singleTransport.classList.add("single-transport")
        singleTransport.name = "transport"
        singleTransport.value = this.type

        var lbl = document.createElement("label")
        lbl.innerHTML = this.type

        host.appendChild(lbl)
        host.appendChild(singleTransport)
    }
}