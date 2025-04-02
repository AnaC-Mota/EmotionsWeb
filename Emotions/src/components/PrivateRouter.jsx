import { useState, useEffect } from "react";
import { Navigate } from "react-router-dom";
import PropTypes from "prop-types";


function PrivateRouter({ children }) {
    const [isAuthorized, setIsAuthorized] = useState(null);

    useEffect(() => {
        const auth = async () => {
            const token = localStorage.getItem("authToken");
            if (!token) {
                setIsAuthorized(false);
            } else {
                setIsAuthorized(true);
            }
        };

        auth(); // Agora a função é chamada corretamente
    }, []);

    if (isAuthorized === null) {
        return <div>Loading...</div>;
    }

    return isAuthorized ? children : <Navigate to="/" />;
}

PrivateRouter.propTypes = {
    children: PropTypes.node.isRequired,
};

export default PrivateRouter;
