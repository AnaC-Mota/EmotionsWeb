import { useState, useEffect, Component } from "react";
import { Route, Navigate } from "react-router-dom";
import { useAuth0 } from "@auth0/auth0-react";

function PrivateRouter({ children, ...rest }) {
    const { isAuthenticated, isLoading } = useAuth0();
    const [isAuthorized, setIsAuthorized] = useState(null)

    useEffect(() => {
        auth().catch(()=>{
            setIsAuthorized(false)
        })
    }, [])

    const auth = async () => {
        const token = localStorage.getItem("authToken")
        if(!token){
            setIsAuthorized(false)
        }
        else{
            setIsAuthorized(true)
        }
    }

    if (isAuthorized == null){
        return <div>Loading...</div>
    }

    return isAuthorized ? children : <Navigate to="/login"></Navigate>
}

export default PrivateRouter;
