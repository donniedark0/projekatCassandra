export class User{
    constructor(id, username, phone, password, rids, cids, res){
        this.id = id
        this.username = username
        this.phone = phone
        this.password = password
        this.ratingIDs = rids
        this.commentIDs = cids
        this.reservations = res
    }

    DrawUser(host){
        
    }
}