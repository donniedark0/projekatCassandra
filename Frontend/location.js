import { DrawHotelInput } from "./main.js"

export class Location{
    constructor(locid, city, state, hotelids, seasonID){
        this.id = locid
        this.city = city
        this.state = state
        this.hotelids = hotelids
        this.seasonID = seasonID
    }

    DrawLocation(host, user){
        var thisptr = this
        var singleLocation = document.createElement("div")
        singleLocation.classList.add("single-location")

        var nameCheckbox = document.createElement("input")
        nameCheckbox.classList.add("name-checkbox")
        nameCheckbox.type = "checkbox"
        nameCheckbox.setAttribute("value", this.id)

        var nameLbl = document.createElement("label")
        nameLbl.innerHTML = this.city + ", " + this.state
        nameLbl.classList.add("location-name")
        var br = document.createElement("br")

        singleLocation.appendChild(nameCheckbox)
        singleLocation.appendChild(nameLbl)
        singleLocation.appendChild(br)

        host.appendChild(singleLocation)

        if(user.id == "1"){
            var deleteLocationBtn = document.createElement("button");
            deleteLocationBtn.classList.add("edit-delete");
            deleteLocationBtn.innerHTML = "&times;"

            deleteLocationBtn.onclick = function(e) {
                e.preventDefault();
                if (confirm("Do you want to delete this item?")) {
                    fetch("https://localhost:5001/Location/DeleteLocation/" + thisptr.seasonID + "/" + thisptr.id, {
                        method: 'DELETE',
                        mode: 'cors'
                    }).then(resp => {
                        if (resp.status == 204) {
                            document.querySelectorAll(".name-checkbox").forEach(p =>{
                                const parent = p.parentNode;
                                if(p.value == thisptr.id){
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
            singleLocation.appendChild(deleteLocationBtn);

            var editLocationBtn = document.createElement("button");
            editLocationBtn.innerHTML = "&#9998;";
            editLocationBtn.classList.add("edit-delete");
            editLocationBtn.onclick = function(e) {
                e.preventDefault();
                var stateName = prompt("New state name:");
                var cityName = prompt("New city name:");
                if (stateName != null && cityName != null) {
                    fetch("https://localhost:5001/Location/EditLocation", {
                        method: 'PUT',
                        mode: 'cors',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({
                            locationID: id,
                            city: cityName,
                            state: stateName
                        })
                    }).then(resp => {
                        if (resp.status == 204) {
                            document.querySelectorAll(".location-name").forEach(el=>{
                                if(el.innerText == thisptr.city + ", " + thisptr.state){
                                    el.innerText = cityName + ", " + stateName
                                    thisptr.city = cityName
                                    thisptr.state = stateName
                                }
                            })
                            alert("Item edited succesfully!");
                        }
                    });
                }
            }
            singleLocation.appendChild(editLocationBtn);


            var addHotelBtn = document.createElement("button")
            addHotelBtn.innerHTML = "Add hotel"

            var numOfHotels = 0
            fetch("https://localhost:5001/Hotel/GetHotelNumber").then(p => {
                p.json().then(data => {
                    numOfHotels = data + 1
                })
            })

            addHotelBtn.onclick = function () {

                var hotelsDiv = document.getElementById("hotels")

                hotelsDiv.innerHTML = ''

                DrawHotelInput(hotelsDiv)

                var submitHotel = document.getElementById("submit-hotel")

                submitHotel.onclick = function () {
                    fetch("https://localhost:5001/Hotel/PostHotel/" + thisptr.id, {
                        method: 'POST',
                        mode: 'cors',
                        headers: {
                            'Content-Type': 'application/json'
                        },
                        body: JSON.stringify({
                            hotelid: numOfHotels.toString(),
                            name: document.getElementById("hotel-name-input").value,
                            phone: document.getElementById("hotel-phone-input").value,
                            picture: document.getElementById("hotel-picture-input").value
                        })
                    }).then(resp => {
                        if (resp.status == 204) {
                            alert("Hotel succesfuly added!")
                            numOfHotels += 1
                        }
                        else{
                            alert("Error")
                        }
                    });
                }


            }

            singleLocation.appendChild(addHotelBtn)
        }

    }
}