import {useState} from "react";
import {Table} from "flowbite-react";
import {FaCheck} from "react-icons/fa";
import DashItem from "./DashItem.jsx";

const DashUsers = () => {
    const [items, setItems] = useState([]);
    const [showMore, setShowMore] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [userIdToDelete, setUserIdToDelete] = useState(null);

    return (
        <DashItem
            fetchUrl="user/get-users"
            setItems={setItems}
            setShowMore={setShowMore}
            itemsLength={items.length}
            showMoreUrl={`user/get-users?startIndex=${items.length}`}
            setShowModal={setShowModal}
            urlToDelete={`user/delete/${userIdToDelete}`}
            itemIdToDelete={userIdToDelete}
            showModal={showModal}
            showMore={showMore}
        >
            <Table.Head>
                <Table.HeadCell>Date created</Table.HeadCell>
                <Table.HeadCell>User image</Table.HeadCell>
                <Table.HeadCell>Username</Table.HeadCell>
                <Table.HeadCell>Email</Table.HeadCell>
                <Table.HeadCell>Admin</Table.HeadCell>
                <Table.HeadCell>Delete</Table.HeadCell>
            </Table.Head>
            {
                items.map((item) =>
                    <Table.Body
                        className="divide-y"
                        key={item.id}
                    >
                        <Table.Row
                            className="bg-white dark:border-gray-700 dark:bg-gray-800"
                        >
                            <Table.Cell>
                                {new Date(item.createdAt).toLocaleDateString()}
                            </Table.Cell>
                            <Table.Cell>
                                <img
                                    src={item.profilePictureUrl}
                                    alt={item.username}
                                    className="w-10 h-10 object-cover bg-gray-500 rounded-full"
                                />
                            </Table.Cell>
                            <Table.Cell>{item.username}</Table.Cell>
                            <Table.Cell>{item.email}</Table.Cell>
                            <Table.Cell>
                                {item.role === "Admin" ? (
                                    <FaCheck className="text-green-500"/>
                                ) : (
                                    <FaCheck className="text-red-500"/>
                                )}
                            </Table.Cell>
                            <Table.Cell>
                                <span
                                    onClick={() => {
                                        setShowModal(true);
                                        setUserIdToDelete(item.id);
                                    }}
                                    className="font-medium text-red-500 hover:underline cursor-pointer"
                                >
                                    Delete
                                </span>
                            </Table.Cell>
                        </Table.Row>
                    </Table.Body>
                )
            }
        </DashItem>
    );
}

export default DashUsers;