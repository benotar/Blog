import {useLocation} from "react-router-dom";
import {useEffect, useState} from "react";
import DashSidebar from "../components/DashSidebar.jsx";
import DashProfile from "../components/DashProfile.jsx";
import DashPost from "../components/DashPost.jsx";
import DashUsers from "../components/DashUsers.jsx";

export default function Dashboard() {
    const location = useLocation();
    const [tab, setTab] = useState("");

    useEffect(() => {
        const urlParams = new URLSearchParams(location.search);
        const tabFromUrl = urlParams.get("tab");
        if (tabFromUrl) {
            setTab(tabFromUrl);
        }
    }, [location.search]);

    return (
        <div className="min-h-screen flex flex-col md:flex-row">
            {/* Sidebar */}
            <div className="md:w-56">
                <DashSidebar/>
            </div>
            {/* Profile... */}
            {tab === "profile" && <DashProfile/>}
            {/* Posts... */}
            {tab === "posts" && <DashPost/>}
            {/* Users... */}
            {tab === "users" && <DashUsers/>}
        </div>
    );
}