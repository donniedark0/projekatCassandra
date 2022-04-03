import { Location } from "./location.js";

export class Season{
    constructor(id, name, locids){
        this.id = id
        this.name = name
        this.locationIDs = locids
    }

    DrawSeason(host, user){
        var thisptr = this
        
        var locationsDiv = document.getElementById("all-locations")
        
        var singleSeason = document.createElement("div")
        singleSeason.classList.add("single-season")

        var seasonBtn = document.createElement("input")
        seasonBtn.classList.add("season-radio")
        seasonBtn.type = "radio"
        seasonBtn.setAttribute("value", this.id)
        seasonBtn.setAttribute("name", "season")

        var nameLbl = document.createElement("label")
        nameLbl.innerHTML = this.name
        nameLbl.classList.add("season-name")

        singleSeason.appendChild(seasonBtn)
        singleSeason.appendChild(nameLbl)
        host.appendChild(singleSeason)

        seasonBtn.onclick = function() {

            while(locationsDiv.firstChild)
            {
                locationsDiv.removeChild(locationsDiv.firstChild)
            }

            fetch("https://localhost:5001/Location/GetLocationsByIDs", {
                method: 'PUT',
                mode: 'cors',
                headers: {
                    'Content-Type': 'application/json'
                },
                body: JSON.stringify(thisptr.locationIDs)
            }).then(resp => {
                resp.json().then(data => {
                    data.forEach(element => {
                        var location = new Location(element.locationID, element.city, element.state, element.hotelIDs, thisptr.id)
                        location.DrawLocation(locationsDiv, user)
                    });
                })
            });
        }

       
        if(user.id == "1"){
            var deleteSeasonBtn = document.createElement("button");
            deleteSeasonBtn.classList.add("edit-delete");
            deleteSeasonBtn.innerHTML = "&times;"
            var id = this.id;

            deleteSeasonBtn.onclick = function(e) {
                e.preventDefault();
                if (confirm("Do you want to delete this item?")) {
                    fetch("https://localhost:5001/Season/DeleteSeason/" + id, {
                        method: 'DELETE',
                        mode: 'cors'
                    }).then(resp => {
                        if (resp.status == 204) {
                            document.querySelectorAll(".season-radio").forEach(p =>{
                                const parent = p.parentNode;
                                if(p.value == id){
                                    while(p.firstChild){
                                        parent.removeChild(parent.firstChild);
                                    }
                                    parent.remove();
                                }
                            })
                            alert("Item sucessfully deleted");
                        }
                    });
                }
            }
            singleSeason.appendChild(deleteSeasonBtn);

            var numOfLocations = 0
            fetch("https://localhost:5001/Location/GetLocationNumber").then(p => {
                p.json().then(data => {
                    numOfLocations = data + 1
                })
            })    

            var editSeasonBtn = document.createElement("button");
            editSeasonBtn.innerHTML = "&#9998;";
            editSeasonBtn.classList.add("edit-delete");
            editSeasonBtn.onclick = function(e) {
                e.preventDefault();
                var newName = prompt("New season name:");
                if (newName != null) {
                    fetch("https://localhost:5001/Season/EditSeason", {
                        method: 'PUT',
                        mode: 'cors',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({
                            seasonID: id,
                            name: newName
                        })
                    }).then(resp => {
                        if (resp.status == 204) {
                            document.querySelectorAll(".season-name").forEach(el=>{
                                if(el.innerText == thisptr.name){
                                    el.innerText = newName
                                    thisptr.name = newName;
                                }
                            })
                            alert("Item edited succesfully!");
                        }
                    });
                }
            }
            singleSeason.appendChild(editSeasonBtn);

            var addLocationBtn = document.createElement("button")
            addLocationBtn.classList.add("add-location-btn")
            addLocationBtn.innerHTML = "Add a location"
            
            addLocationBtn.onclick = function() {
                var state = prompt("State: ")
                var city = prompt("City: ")
                if(state != null && city != null){

                    fetch("https://localhost:5001/Location/PostLocation/" + thisptr.id, {
                        method: 'POST',
                        mode: 'cors',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({
                            locationID: numOfLocations.toString(),
                            city: city,
                            state: state
                        })
                    }).then(resp => {
                        if (resp.status == 204) {
                            const location = new Location(numOfLocations, city, state, null, thisptr.id)
                            location.DrawLocation(locationsDiv, user)
                            alert("Location succesfuly added!")
                        }
                        else{
                            alert("Error")
                        }
                    });
                }
            }

            host.appendChild(addLocationBtn)
        }
    }
}