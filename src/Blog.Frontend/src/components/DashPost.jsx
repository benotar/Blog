import {useState} from "react";
import {useAppStore} from "../zustand/useAppStore.js";
import {Table} from "flowbite-react";
import {Link} from "react-router-dom";
import DashItem from "./DashItem.jsx";


const DashPost = () => {
    const currentUser = useAppStore((state) => state.currentUser);
    const [items, setItems] = useState([]);
    const [showMore, setShowMore] = useState(true);
    const [showModal, setShowModal] = useState(false);
    const [postIdToDelete, setPostIdToDelete] = useState(null);

    return (
        <DashItem
            fetchUrl={`post/get-posts?userId=${currentUser.id}`}
            setItems={setItems}
            setShowMore={setShowMore}
            itemsLength={items.length}
            showMoreUrl={`post/get-posts?userId=${currentUser.id}&startIndex=${items.length}`}
            setShowModal={setShowModal}
            urlToDelete={`post/delete-post/${postIdToDelete}/${currentUser.id}`}
            itemIdToDelete={postIdToDelete}
            showModal={showModal}
            showMore={showMore}
        >
            <Table.Head>
                <Table.HeadCell>Date updated</Table.HeadCell>
                <Table.HeadCell>Post image</Table.HeadCell>
                <Table.HeadCell>Post title</Table.HeadCell>
                <Table.HeadCell>Post category</Table.HeadCell>
                <Table.HeadCell>Delete</Table.HeadCell>
                <Table.HeadCell><span>Edit</span></Table.HeadCell>
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
                                {new Date(item.updatedAt).toLocaleDateString()}
                            </Table.Cell>
                            <Table.Cell>
                                <Link to={`/post/${item.slug}`}>
                                    <img
                                        src={item.imageUrl}
                                        alt={item.title}
                                        className="w-20 h-20 object-cover bg-gray-500"
                                    />
                                </Link>
                            </Table.Cell>
                            <Table.Cell>
                                <Link
                                    to={`/post/${item.slug}`}
                                    className="font-medium text-gray-900 dark:text-white"
                                >
                                    {item.title}
                                </Link>
                            </Table.Cell>
                            <Table.Cell>
                                <Link to={`/post/${item.slug}`}>
                                    {item.category}
                                </Link>
                            </Table.Cell>
                            <Table.Cell>
                                    <span
                                        onClick={() => {
                                            setShowModal(true);
                                            setPostIdToDelete(item.id);
                                        }}
                                        className="font-medium text-red-500 hover:underline cursor-pointer"
                                    >
                                        Delete
                                    </span>
                            </Table.Cell>
                            <Table.Cell>
                                <Link
                                    to={`/update-post/${item.id}`}
                                    className="text-teal-500 hover:underline"
                                >
                                    <span>
                                        Edit
                                    </span>
                                </Link>
                            </Table.Cell>
                        </Table.Row>
                    </Table.Body>
                )
            }
        </DashItem>
    );

};

export default DashPost;