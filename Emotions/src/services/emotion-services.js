import {APIService}  from "../http-common";

class EmotionService{
    create(data){
        return APIService.Axios().post("Home/AddDocument", data);
    }
    get(){
        return APIService.Axios().get("Home/GetAllDocuments");
    }
}

export default new EmotionService();